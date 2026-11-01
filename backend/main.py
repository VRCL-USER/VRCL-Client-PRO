from __future__ import annotations

import hmac
import os
import sqlite3
from contextlib import closing
from datetime import datetime, timezone
from pathlib import Path
from typing import Annotated

from dotenv import load_dotenv
from fastapi import Depends, FastAPI, Header, HTTPException, status
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel, Field

load_dotenv()

DATA_DIR = Path(os.getenv("VRCL_DATA_DIR", "./data")).resolve()
DATA_DIR.mkdir(parents=True, exist_ok=True)
DB_PATH = DATA_DIR / "vrcl_catalog.sqlite3"
API_TOKEN = os.getenv("API_TOKEN", "").strip()
CORS_ORIGINS = [
    origin.strip()
    for origin in os.getenv("CORS_ORIGINS", "http://127.0.0.1:5500").split(",")
    if origin.strip()
]

app = FastAPI(
    title="VRCL Client PRO API",
    version="0.1.0",
    description="Local-first service for authorized asset metadata and agent status.",
)

app.add_middleware(
    CORSMiddleware,
    allow_origins=CORS_ORIGINS,
    allow_credentials=False,
    allow_methods=["GET", "POST"],
    allow_headers=["Authorization", "Content-Type"],
)


def connect_db() -> sqlite3.Connection:
    connection = sqlite3.connect(DB_PATH)
    connection.row_factory = sqlite3.Row
    return connection


def initialize_database() -> None:
    with closing(connect_db()) as connection:
        connection.executescript(
            """
            CREATE TABLE IF NOT EXISTS agent_heartbeats (
                agent_id TEXT PRIMARY KEY,
                hostname TEXT NOT NULL,
                platform TEXT NOT NULL,
                last_seen_utc TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS assets (
                asset_id TEXT PRIMARY KEY,
                display_name TEXT NOT NULL,
                avatar_id TEXT,
                local_path TEXT,
                authorized INTEGER NOT NULL DEFAULT 0,
                authorization_note TEXT NOT NULL DEFAULT '',
                imported_at_utc TEXT NOT NULL
            );
            """
        )
        connection.commit()


initialize_database()


def require_api_token(
    authorization: Annotated[str | None, Header()] = None,
) -> None:
    if not API_TOKEN:
        raise HTTPException(
            status_code=status.HTTP_503_SERVICE_UNAVAILABLE,
            detail="API_TOKEN is not configured on the server.",
        )

    scheme, separator, supplied_token = (authorization or "").partition(" ")
    if (
        scheme.lower() != "bearer"
        or not separator
        or not hmac.compare_digest(supplied_token, API_TOKEN)
    ):
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="A valid Bearer token is required.",
            headers={"WWW-Authenticate": "Bearer"},
        )


class AgentHeartbeat(BaseModel):
    agent_id: str = Field(min_length=1, max_length=100)
    hostname: str = Field(min_length=1, max_length=255)
    platform: str = Field(default="Windows", max_length=100)


@app.get("/health")
def health() -> dict[str, str]:
    return {
        "status": "ok",
        "service": "vrcl-client-pro-api",
        "version": app.version,
    }


@app.post("/v1/agent/heartbeat", dependencies=[Depends(require_api_token)])
def agent_heartbeat(payload: AgentHeartbeat) -> dict[str, str]:
    now = datetime.now(timezone.utc).isoformat()
    with closing(connect_db()) as connection:
        connection.execute(
            """
            INSERT INTO agent_heartbeats (agent_id, hostname, platform, last_seen_utc)
            VALUES (?, ?, ?, ?)
            ON CONFLICT(agent_id) DO UPDATE SET
                hostname = excluded.hostname,
                platform = excluded.platform,
                last_seen_utc = excluded.last_seen_utc
            """,
            (payload.agent_id, payload.hostname, payload.platform, now),
        )
        connection.commit()

    return {"status": "received", "agent_id": payload.agent_id, "last_seen_utc": now}


@app.get("/v1/agent/status", dependencies=[Depends(require_api_token)])
def agent_status() -> dict[str, list[dict[str, str]]]:
    with closing(connect_db()) as connection:
        rows = connection.execute(
            """
            SELECT agent_id, hostname, platform, last_seen_utc
            FROM agent_heartbeats
            ORDER BY last_seen_utc DESC
            """
        ).fetchall()

    return {"agents": [dict(row) for row in rows]}


@app.get("/v1/assets", dependencies=[Depends(require_api_token)])
def list_authorized_assets() -> dict[str, list[dict[str, str | None]]]:
    # Only records explicitly marked authorized are ever returned.
    with closing(connect_db()) as connection:
        rows = connection.execute(
            """
            SELECT asset_id, display_name, avatar_id, imported_at_utc
            FROM assets
            WHERE authorized = 1
            ORDER BY imported_at_utc DESC
            """
        ).fetchall()

    return {"assets": [dict(row) for row in rows]}


@app.get("/")
def root() -> dict[str, str]:
    return {"message": "VRCL Client PRO API is running. See /docs for the local API schema."}
