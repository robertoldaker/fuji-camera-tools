#
app="ImageViewer.server"

function raiseError()
{
    echo "Press any key to continue";
    read
    exit -1;
}

# Check version control and also generate VersionData.cs before publishing
python ../Scripts/CheckVersion.py .. VersionData.csx VersionData.cs
if [ $? -ne 0 ]; then
    raiseError;
fi

echo "Deleting old ASP.NET build ..."
rm -r $HOME/ImageViewer
echo "Building app .."
dotnet publish $app.csproj -o "$HOME/ImageViewer" -c "RELEASE"
if [ $? -ne 0 ]; then
    raiseError;
fi
echo "Restarting ImageViewer service"
sudo systemctl restart ImageViewer

echo "Build & publish successful. Press any key to continue";
read


