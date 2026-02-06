dotnet publish -r win-x64 --self-contained true -c Release -o ClientApp/build-neo-node
cd ClientApp
npm ci
CSC_IDENTITY_AUTO_DISCOVERY=false npm run publish