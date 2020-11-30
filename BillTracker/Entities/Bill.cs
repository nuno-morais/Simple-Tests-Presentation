namespace BillTracker.Entities
{
    public class Bill
    {
        public Bill()
        {
        }

        public int Id { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (obj is not Bill) return false;

            var value = obj as Bill;
            return (value.Id == Id &&
                value.Description == Description &&
                value.Price == Price);
        }
    }
}
