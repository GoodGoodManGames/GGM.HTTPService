namespace GGM.Web.View
{
    public class ViewModel
    {
        public static ViewModel Get(string url) => new ViewModel(url);

        public ViewModel(string url) : this(url, null)
        {
        }

        public ViewModel(string url, object model)
        {
            URL = url;
            Model = model;
        }

        public string URL { get; }
        public object Model { get; private set; }

        public ViewModel SetModel(object model)
        {
            Model = model;
            return this;
        }
    }
}