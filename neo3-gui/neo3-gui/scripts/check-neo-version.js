const fs = require('fs');
const path = require('path');

const expectedVersion = '3.9.1';
const csprojPath = path.join(__dirname, '..', 'neo3-gui.csproj');

if (!fs.existsSync(csprojPath)) {
  console.error(`Missing csproj at ${csprojPath}`);
  process.exit(1);
}

const content = fs.readFileSync(csprojPath, 'utf8');
const match = content.match(/<PackageReference\s+Include="Neo"\s+Version="([^"]+)"\s*\/?>/);
if (!match) {
  console.error('Neo PackageReference not found');
  process.exit(1);
}

const actualVersion = match[1];
if (actualVersion !== expectedVersion) {
  console.error(`Neo version mismatch: expected ${expectedVersion}, found ${actualVersion}`);
  process.exit(1);
}

console.log(`Neo version OK: ${actualVersion}`);
