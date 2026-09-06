import tempfile
import unittest
from pathlib import Path

from PIL import Image
from resize_asset import resize_asset


class ResizeAssetTests(unittest.TestCase):
    def test_grid_aspect_padding_and_preservation(self):
        with tempfile.TemporaryDirectory() as folder:
            root = Path(folder)
            source, reference, output = (root / n for n in ("source.png", "ref.png", "out.png"))
            Image.new("RGBA", (80, 40), (255, 0, 0, 255)).save(source)
            Image.new("RGBA", (40, 40)).save(reference)
            before = source.read_bytes()
            resize_asset(source, output, reference, 2)
            with Image.open(output) as result:
                self.assertEqual(result.size, (40, 40))
                self.assertEqual(result.mode, "RGBA")
                self.assertEqual(result.getbbox(), (0, 10, 40, 30))
                for y in range(0, 40, 2):
                    for x in range(0, 40, 2):
                        self.assertEqual(len({result.getpixel((x+dx, y+dy))
                                              for dx in range(2) for dy in range(2)}), 1)
            self.assertEqual(source.read_bytes(), before)
            with self.assertRaises(ValueError):
                resize_asset(source, output, reference)
            with self.assertRaises(ValueError):
                resize_asset(source, source, reference)
            for pixel_size in (0, -1, 3):
                with self.assertRaises(ValueError):
                    resize_asset(source, root / "bad.png", reference, pixel_size)
            Image.new("RGBA", (10, 10)).save(source)
            with self.assertRaises(ValueError):
                resize_asset(source, root / "empty.png", reference)


if __name__ == "__main__":
    unittest.main()
