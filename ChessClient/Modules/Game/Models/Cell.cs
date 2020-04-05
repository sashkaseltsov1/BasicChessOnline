using Prism.Mvvm;

namespace ChessClient.Modules.Game.ViewModels
{
    public class Cell :BindableBase
    {
        public string Source
        {
            get { return _source; }
            set { SetProperty(ref _source, value); }
        }

        public string ValidCell
        {
            get { return _validCell; }
            set { SetProperty(ref _validCell, value); }
        }

        public string Color { get; set; }

        public string Position { get; set; }

        private string _source;

        private string _validCell;

        public Cell(string color)
        {
            Color = color;
            ValidCell = "Hidden";
        }
    }
}
