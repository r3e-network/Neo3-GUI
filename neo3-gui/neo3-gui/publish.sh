node scripts/fetch-leveldbstore.js
dotnet publish -c Release -o ClientApp/build-neo-node
cd ClientApp
npm install
npm run publish
