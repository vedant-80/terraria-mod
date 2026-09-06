#!/usr/bin/env python3
"""Fit a single sprite to a reference canvas using square logical pixels."""
import argparse
from pathlib import Path


def resize_asset(source, destination, reference, pixel_size=1):
    from PIL import Image

    source, destination, reference = map(Path, (source, destination, reference))
    if destination.exists() or destination.resolve() in (source.resolve(), reference.resolve()):
        raise ValueError("Destination must be a new file; source and reference are preserved")
    if destination.suffix.lower() != ".png":
        raise ValueError("Destination must be a lossless .png")
    with Image.open(reference) as ref:
        width, height = ref.size
    if pixel_size < 1 or width % pixel_size or height % pixel_size:
        raise ValueError("Pixel size must be positive and divide both reference dimensions")
    with Image.open(source) as original:
        if getattr(original, "n_frames", 1) != 1:
            raise ValueError("Animated inputs must be exported frame by frame")
        sprite = original.convert("RGBA")
    bounds = sprite.getbbox()
    if bounds is None:
        raise ValueError("Source is fully transparent")
    sprite = sprite.crop(bounds)
    grid_width, grid_height = width // pixel_size, height // pixel_size
    scale = min(grid_width / sprite.width, grid_height / sprite.height)
    fitted = (max(1, min(grid_width, round(sprite.width * scale))),
              max(1, min(grid_height, round(sprite.height * scale))))
    canvas = Image.new("RGBA", (grid_width, grid_height))
    canvas.paste(sprite.resize(fitted, Image.Resampling.NEAREST),
                 ((grid_width - fitted[0]) // 2, (grid_height - fitted[1]) // 2))
    canvas = canvas.resize((width, height), Image.Resampling.NEAREST)
    # Exclusive creation also prevents accidental overwrites between checking and saving.
    with destination.open("xb") as output:
        canvas.save(output, format="PNG")
    return {"canvas": (width, height), "logical_content": fitted, "pixel_size": pixel_size}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("source", type=Path)
    parser.add_argument("destination", type=Path)
    parser.add_argument("--reference", required=True, type=Path,
                        help="Comparable single-frame ExampleMod PNG (dimensions only)")
    parser.add_argument("--pixel-size", type=int, default=1,
                        help="Square exported pixels per logical pixel; default 1")
    args = parser.parse_args()
    try:
        print(resize_asset(args.source, args.destination, args.reference, args.pixel_size))
    except (ValueError, OSError, ImportError) as error:
        parser.exit(1, f"Resize failed: {error}\nInstall Pillow using harness/helpers/requirements.txt if missing.\n")


if __name__ == "__main__":
    main()
