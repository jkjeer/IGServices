using System.Collections.Generic;
using System.Data;

namespace IGServices.Common.Types.Response
{

    public class ReaderSet
    {

        public Dictionary<string, int> Columns { get; set; }
        public List<object[]> Rows { get; set; }
        public object[] CurrentRow { get; set; }

        public ReaderSet()
        {
            Columns = new Dictionary<string, int>();
            Rows = new List<object[]>();
        }


        public void Fill(IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                Columns.Add(reader.GetName(i), i);
            }

            while (reader.Read())
            {
                var row = new object[Columns.Count];
                reader.GetValues(row);
                Rows.Add(row);
            }            
        }
    }
}
