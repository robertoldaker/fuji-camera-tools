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

# now uses script ImageViewer.sh to start/stop
# this is added to startup application to get the program to run at boot
echo "Restarting ImageViewer"
~/ImageViewer/ImageViewer.sh restart

echo "Build & publish successful. Press any key to continue";
read


