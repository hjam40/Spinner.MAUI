using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spinner.MAUI.Test
{
    internal class SpinnerItem : ISpinnerItem
    {
        public ImageSource ImageSource { get; set; }
        public string Text { get; set; } = string.Empty;
        public object Item { get; set; }
    }
}
