import base64
import pyperclip
# Read the file in binary mode
with open('C:\Program Files (x86)\Steam\config.vdf', 'rb') as file:
    binary_data = file.read()

# Encode the binary data to base64
base64_data = base64.b64encode(binary_data)

# Print or save the base64 data
print(base64_data)
pyperclip.copy(base64_data.decode())