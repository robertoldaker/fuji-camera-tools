using ImageViewer.shared;

namespace ImageViewer.server;

public class VersionData : VersionDataBase {

    public VersionData() {
        Version = "1.0";
        CommitId = "$COMMIT_ID$";
        CommitDate = "$COMMIT_DATE$";
    }

}