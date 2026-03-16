I need to manage downloads and downloaded audio files, maybe also imported files.
I could do it in a way, where it is two classes:
1. DownloadItem: manages a download, has a property/foreign key that links it to a downloaded file
2. MusicFile: The file on the disk, does not depend on being a downloadItem

I could convert the donwloadItem to be a MusicFile after it fully downloads, or I could create a MusicFile to correspond to a DownloadItem from the start

IS the AudioMeta enough?