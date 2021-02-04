using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expert_VSR
{
    public class Main : IEnumerable<object>
    {
        public List<object> _val { get; private set; }

        public Main()
        {

        }
        public Main(IDataRecord record)
        {
            Init(record);
        }
        public void Init(IDataRecord record)
        {
            _val = new List<object>();

            for (int i = 0; i < record.FieldCount; i++)
            {
                _val.Add(record[i]);
            }
        }

        public IEnumerator<object> GetEnumerator()
        {
            return ((IEnumerable<object>)_val).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<object>)_val).GetEnumerator();
        }
    }
}
