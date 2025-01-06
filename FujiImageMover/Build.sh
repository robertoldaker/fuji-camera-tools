#
app="FujiImageMover"

function raiseError()
{
    echo "Press any key to continue";
    read
    exit -1;
}

echo "Building .deb .."
dotnet deb $app.csproj -c Release  -o ./build
if [ $? -ne 0 ]; then
    raiseError;
fi


echo "Build successful. Press any key to continue";
read