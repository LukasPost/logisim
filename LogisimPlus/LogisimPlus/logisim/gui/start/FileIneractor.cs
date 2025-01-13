using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisimPlus.logisim.gui.start;
public class FileInteractor
{
    public string Path {  get; set; }

    public FileInteractor(string path)
    {
        Path = path;
    }

    public StreamReader GetStreamReader()
    {
        return new StreamReader(Path);
    }

    public StreamWriter GetStreamWriter() { return new StreamWriter(Path); }
}
