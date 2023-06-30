using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ILoading
{
    public void Load(bool fullAlpha = true);

    public void Unload();
    public bool isloading();
}