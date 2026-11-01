from __future__ import annotations

import os

import discord
import httpx
from discord import app_commands
from discord.ext import commands
from dotenv import load_dotenv

load_dotenv()

DISCORD_TOKEN = os.getenv("DISCORD_TOKEN", "").strip()
API_BASE_URL = os.getenv("API_BASE_URL", "http://127.0.0.1:8000").rstrip("/")
API_TOKEN = os.getenv("API_TOKEN", "").strip()
GUILD_ID = os.getenv("DISCORD_GUILD_ID", "").strip()

intents = discord.Intents.none()
bot = commands.Bot(command_prefix="!", intents=intents)


@bot.event
async def on_ready() -> None:
    print(f"Signed in as {bot.user} (ID: {bot.user.id if bot.user else 'unknown'})")
    try:
        if GUILD_ID:
            guild = discord.Object(id=int(GUILD_ID))
            bot.tree.copy_global_to(guild=guild)
            synced = await bot.tree.sync(guild=guild)
            print(f"Synced {len(synced)} command(s) to test guild {GUILD_ID}.")
        else:
            synced = await bot.tree.sync()
            print(f"Synced {len(synced)} global command(s).")
    except (discord.HTTPException, ValueError) as exc:
        print(f"Command sync failed: {exc}")


@bot.tree.command(name="status", description="Check whether the VRCL API is reachable.")
async def status_command(interaction: discord.Interaction) -> None:
    await interaction.response.defer(ephemeral=True)
    try:
        async with httpx.AsyncClient(timeout=8.0) as client:
            response = await client.get(f"{API_BASE_URL}/health")
            response.raise_for_status()
            payload = response.json()
        await interaction.followup.send(
            f"API status: **{payload.get('status', 'unknown')}** "
            f"({payload.get('service', 'service')}, v{payload.get('version', '?')})",
            ephemeral=True,
        )
    except (httpx.HTTPError, ValueError) as exc:
        await interaction.followup.send(
            f"The API could not be reached: {exc}",
            ephemeral=True,
        )


@bot.tree.command(name="assets", description="List assets explicitly marked as authorized.")
async def assets_command(interaction: discord.Interaction) -> None:
    await interaction.response.defer(ephemeral=True)

    if not API_TOKEN:
        await interaction.followup.send(
            "The API_TOKEN is not configured. Set it in the local .env file.",
            ephemeral=True,
        )
        return

    try:
        async with httpx.AsyncClient(timeout=8.0) as client:
            response = await client.get(
                f"{API_BASE_URL}/v1/assets",
                headers={"Authorization": f"Bearer {API_TOKEN}"},
            )
            response.raise_for_status()
            payload = response.json()

        assets = payload.get("assets", [])
        if not assets:
            message = "The authorized asset catalog is empty."
        else:
            lines = [
                f"• **{item.get('display_name', 'Unnamed asset')}** "
                f"— ID: `{item.get('avatar_id') or 'not set'}`"
                for item in assets[:15]
            ]
            message = "**Authorized assets**\n" + "\n".join(lines)
            if len(assets) > 15:
                message += f"\n…and {len(assets) - 15} more."

        await interaction.followup.send(message, ephemeral=True)
    except (httpx.HTTPError, ValueError) as exc:
        await interaction.followup.send(
            f"Could not load the asset catalog: {exc}",
            ephemeral=True,
        )


if __name__ == "__main__":
    if not DISCORD_TOKEN:
        raise SystemExit("DISCORD_TOKEN is missing. Add it to your local .env file.")
    bot.run(DISCORD_TOKEN)
