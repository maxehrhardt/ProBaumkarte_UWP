using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProBaumkarte_UWP.Models.Map
{
    public class MapAndTrees
    {
        public MapFile map;
        public ObservableCollection<ProBaumkarte_UWP.Models.Baum.Baum> baumListe;
    }
}
