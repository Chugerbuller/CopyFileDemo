namespace FileHelperService;

public class FileHelper
{
    public IEnumerable<FileInfo> Files { get; private set; }
    public readonly string Path;

    public FileHelper(string path)
    {
        if (_isExist(path))
        {
            Path = path;
            _getFiles();
        }
        else
            throw new IOException($"{nameof(Path)} this directory is not exist");
    }

    public void MoveToOtherDirectory(string newPath)
    {
        if (!_isExist(newPath))
        {
            Directory.CreateDirectory(newPath);
        }
        foreach (var file in Files)
        {
            var fileName = file.Name; // test.txt testCopy.txt
            var extension = file.Extension;
            var copyFileName = fileName.Replace(extension, $"Copy{extension}");
            file.CopyTo($"{newPath}\\{copyFileName}");
        }
    }

    public void MoveToOtherDirectoryParallel(string newPath, IProgress<int> progress)
    {
        if (!_isExist(newPath))
        {
            Directory.CreateDirectory(newPath);
        }
        Parallel.ForEach(Files, file =>
         {
           var newPathFile = _createCopyFileName(file, newPath);

             if (File.Exists(newPathFile))
                 File.Delete(newPathFile);

             file.CopyTo(newPathFile);
             progress.Report(1);
         });
    }

    public async Task MoveToOtherDirectoryParallelAsync(string newPath, IProgress<int> progress)
    {
        if (!_isExist(newPath))
        {
            Directory.CreateDirectory(newPath);
        }

        await Parallel.ForEachAsync(Files, async (file, ct) =>
        {
            var newPathFile = _createCopyFileName(file, newPath);

            if (File.Exists(newPathFile))
                File.Delete(newPathFile);

            using (Stream source = file.OpenRead())
            {
                using (Stream destination = File.Create(newPathFile))
                {
                    await source.CopyToAsync(destination, ct);
                }
            }
            progress.Report(1);
        });
    }

    private string _createCopyFileName(FileInfo file, string newPath)
    {

        var fileName = file.Name;
        var extension = file.Extension;
        var copyFileName = fileName.Replace(extension, $"(Copy){extension}");

        return $"{newPath}\\{copyFileName}";
    }

    private void _getFiles()
    {
        var dir = new DirectoryInfo(Path);

        if (dir == null)
            throw new IOException("Directory is empty");

        Files = dir.GetFiles();
    }

    private bool _isExist(string path)
    {
        return Directory.Exists(path);
    }
}
