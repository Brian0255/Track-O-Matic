#!/bin/bash

# Cross-platform script to update AutoUpdateInfo.xml with version information

VERSION="${1:-2.1.8}"
OUTPUT_PATH="${2:-AutoUpdateInfo.xml}"

# Check if the output file exists
if [ ! -f "$OUTPUT_PATH" ]; then
	echo "Error: $OUTPUT_PATH not found"
	exit 1
fi

# Use a temporary file to avoid issues with in-place editing
TEMP_FILE="${OUTPUT_PATH}.tmp"

# Update the version in the XML file
# This approach handles the XML parsing in a cross-platform way
sed "s|<version>.*</version>|<version>$VERSION</version>|g" "$OUTPUT_PATH" > "$TEMP_FILE"

# Update the URL to include the version
sed -i.bak "s|<url>.*</url>|<url>https://github.com/Brian0255/Track-O-Matic/releases/download/$VERSION/TrackOMatic_$VERSION.zip</url>|g" "$TEMP_FILE"

# Remove the backup file created by sed -i
rm -f "${TEMP_FILE}.bak"

# Move the temp file to replace the original
mv "$TEMP_FILE" "$OUTPUT_PATH"

# Ensure file ends with a newline
echo "" >> "$OUTPUT_PATH"

echo "Updated AutoUpdateInfo.xml to version $VERSION"
