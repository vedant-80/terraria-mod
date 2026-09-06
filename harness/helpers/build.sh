#!/usr/bin/env bash
# Run from any directory. Return dotnet's status unchanged.
set -euo pipefail
repo_root="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")/../.." && pwd)"
cd "$repo_root"
sdk_version="$(dotnet --version)"
if [[ "$sdk_version" != 8.* ]]; then
    echo "Build requires .NET SDK 8; selected: $sdk_version" >&2
    exit 2
fi
exec dotnet build "$repo_root/NewMod.csproj" --nologo
