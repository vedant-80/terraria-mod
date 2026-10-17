"""Check the repository's conventional weapon source/PNG pairs (not a C# parser)."""
from pathlib import Path
import struct
import sys


def check_weapon_assets(root):
    errors = []
    sources = list((root / "Content/Items/Weapons").rglob("*.cs"))
    sources.extend((root / "Content/Projectiles").rglob("*.cs"))
    for source in sorted(sources):
        texture = source.with_suffix(".png")
        # Enumerating names also catches incorrect casing on case-insensitive systems.
        if texture.name not in {p.name for p in source.parent.iterdir()}:
            errors.append(f"Missing content texture: {texture.relative_to(root)}")
            continue
        try:
            header = texture.read_bytes()[:24]
            if (len(header) != 24 or header[:8] != b"\x89PNG\r\n\x1a\n"
                    or header[12:16] != b"IHDR"
                    or 0 in struct.unpack(">II", header[16:24])):
                errors.append(f"Invalid PNG header: {texture.relative_to(root)}")
        except OSError as error:
            errors.append(f"Unreadable weapon texture: {texture}: {error}")
    return errors


if __name__ == "__main__":
    failures = check_weapon_assets(Path(__file__).resolve().parents[2])
    for failure in failures:
        print(failure, file=sys.stderr)
    if not failures:
        print("Weapon/projectile sprite paths and PNG headers: PASS")
    sys.exit(bool(failures))
