from __future__ import annotations

import asyncio
import os
import platform
import socket
import uuid

import httpx
from dotenv import load_dotenv

load_dotenv()

API_BASE_URL = os.getenv("API_BASE_URL", "http://127.0.0.1:8000").rstrip("/")
API_TOKEN = os.getenv("API_TOKEN", "").strip()
AGENT_ID = os.getenv("AGENT_ID", "").strip() or socket.gethostname()
try:
    HEARTBEAT_INTERVAL = max(10, int(os.getenv("HEARTBEAT_INTERVAL_SECONDS", "30")))
except ValueError:
    HEARTBEAT_INTERVAL = 30


async def run_agent() -> None:
    if not API_TOKEN:
        raise SystemExit("API_TOKEN is missing. Add it to your local .env file.")

    payload = {
        "agent_id": AGENT_ID,
        "hostname": socket.gethostname(),
        "platform": f"{platform.system()} {platform.release()}",
    }
    headers = {"Authorization": f"Bearer {API_TOKEN}"}

    print(f"VRCL companion agent started as {AGENT_ID}. Press Ctrl+C to stop.")
    print("This starter agent sends heartbeat metadata only; it does not scan or upload files.")

    async with httpx.AsyncClient(timeout=10.0) as client:
        while True:
            try:
                response = await client.post(
                    f"{API_BASE_URL}/v1/agent/heartbeat",
                    json=payload,
                    headers=headers,
                )
                response.raise_for_status()
                print("Heartbeat accepted.")
            except httpx.HTTPError as exc:
                print(f"Heartbeat failed: {exc}")

            await asyncio.sleep(HEARTBEAT_INTERVAL)


if __name__ == "__main__":
    try:
        asyncio.run(run_agent())
    except KeyboardInterrupt:
        print("\nAgent stopped.")
