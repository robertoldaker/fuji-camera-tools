#
#dest="roberto@lv-app.net-zero-energy-systems.org"
app="ImageViewer.server"

function raiseError()
{
    echo "Press any key to continue";
    read
    exit -1;
}

echo "Deleting old ASP.NET build ..."
rm -r $HOME/ImageViewer
echo "Building app .."
dotnet publish $app.csproj -o "$HOME/ImageViewer" -c "RELEASE"
if [ $? -ne 0 ]; then
    raiseError;
fi
sudo systemctl restart ImageViewer

echo "Build & publish successful. Press any key to continue";
read


