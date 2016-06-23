namespace CM.Geo.BL
{
    public class GeoUser
    {
        public string Name { get; internal set; }

        public override string ToString()
        {
            return Name;
        }
    }
}