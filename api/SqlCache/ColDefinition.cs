using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlCache
{
    public class ColDefinition
    {

        public enum DataTypes
        {
            VARCHAR,
            TEXT,
            BIGINT,
            INT,
            SMALLINT,
            BIT,
            DATE,
            DATETIME,
            DECIMAL,
            FLOAT,
            UNIQUEIDENTIFIER
        }

        public string ColumnName { get; set; }

        public DataTypes DataType { get; set; }

        public long CharactersLength { get; set; }

        public bool Nullable { get; set; }

    }
}
