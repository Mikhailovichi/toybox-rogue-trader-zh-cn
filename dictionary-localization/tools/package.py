"""Create a dictionary-only archive with paths relative to the ToyBox mod root."""

import json
from zipfile import ZIP_DEFLATED, ZipFile

from validate import PROJECT, validate

FILES = (
    "Localization/zh-CN_lang.json", "README.md", "CHANGELOG.md", "CREDITS.md", "LICENSE.txt",
)


def package():
    report = validate()
    target = PROJECT.parent / "release/toybox-zh-cn-v2.0.zip"
    target.parent.mkdir(parents=True, exist_ok=True)
    with ZipFile(target, "w", compression=ZIP_DEFLATED, compresslevel=9) as archive:
        for name in FILES:
            archive.write(PROJECT / name, name)
    with ZipFile(target) as archive:
        assert set(archive.namelist()) == set(FILES), "Unexpected archive content"
        for name in FILES:
            assert archive.read(name) == (PROJECT / name).read_bytes(), name
        archive_data = json.loads(archive.read("Localization/zh-CN_lang.json"))
        assert len(archive_data) == report["total_entries"]
    print(f"Created {target} ({target.stat().st_size} bytes)")


if __name__ == "__main__":
    package()
