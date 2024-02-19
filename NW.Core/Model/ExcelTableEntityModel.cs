using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Model
{
    public class ExcelTableEntityModel : ITableEntity
    
    {
        #region ITableEntity properties
        // Summary:
        //     Gets or sets the entity's current ETag. Set this value to '*' in order to
        //     blindly overwrite an entity as part of an update operation.
        public string ETag { get; set; }
        //
        // Summary:
        //     Gets or sets the entity's partition key.
        public string PartitionKey { get; set; }
        //
        // Summary:
        //     Gets or sets the entity's row key.
        public string RowKey { get; set; }
        //
        // Summary:
        //     Gets or sets the entity's time stamp.
        public DateTimeOffset Timestamp { get; set; }
        #endregion

        // Use this Dictionary store table's properties. 
        public IDictionary<string, EntityProperty> Properties { get; private set; }

        public ExcelTableEntityModel()
        {
            Properties = new Dictionary<string, EntityProperty>();
        }

        public ExcelTableEntityModel(string PartitionKey, string RowKey)
        {
            this.PartitionKey = PartitionKey;
            this.RowKey = RowKey;
            Properties = new Dictionary<string, EntityProperty>();
        }

        #region ITableEntity implementation

        public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            this.Properties = properties;
        }

        public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            return this.Properties;
        }

        #endregion

        /// <summary>
        /// Convert object value to EntityProperty.
	    /// </summary>
        static public EntityProperty GetEntityProperty(string key, object value)
        {
            if (value == null)
                return new EntityProperty((string)null);
            if (value.GetType() == typeof(byte[]))
                return new EntityProperty((byte[])value);
            if (value.GetType() == typeof(bool))
                return new EntityProperty((bool)value);
            if (value.GetType() == typeof(DateTimeOffset))
                return new EntityProperty((DateTimeOffset)value);
            if (value.GetType() == typeof(DateTime))
                return new EntityProperty((DateTime)value);
            if (value.GetType() == typeof(double))
                return new EntityProperty((double)value);
            if (value.GetType() == typeof(Guid))
                return new EntityProperty((Guid)value);
            if (value.GetType() == typeof(int))
                return new EntityProperty((int)value);
            if (value.GetType() == typeof(long))
                return new EntityProperty((long)value);
            if (value.GetType() == typeof(string))
                return new EntityProperty((string)value);
            throw new Exception(string.Concat(new object[]
            {
                "not supported ",
                value.GetType(),
                " for ",
                key
            }));
        }

        /// <summary>
        /// Get the edm type, if the type is not a edm type throw a exception.
        /// </summary>
        static public Type GetType(EdmType edmType)
        {
            switch (edmType)
            {
                case EdmType.Binary:
                    return typeof(byte[]);
                case EdmType.Boolean:
                    return typeof(bool);
                case EdmType.DateTime:
                    return typeof(DateTime);
                case EdmType.Double:
                    return typeof(double);
                case EdmType.Guid:
                    return typeof(Guid);
                case EdmType.Int32:
                    return typeof(int);
                case EdmType.Int64:
                    return typeof(long);
                case EdmType.String:
                    return typeof(string);
                default:
                    throw new Exception("not supported " + edmType);
            }
        }

        static private object GetValue(EntityProperty property)
        {
            switch (property.PropertyType)
            {
                case EdmType.String:
                    return property.StringValue;
                case EdmType.Binary:
                    return property.BinaryValue;
                case EdmType.Boolean:
                    return property.BooleanValue;
                case EdmType.DateTime:
                    return property.DateTimeOffsetValue;
                case EdmType.Double:
                    return property.DoubleValue;
                case EdmType.Guid:
                    return property.GuidValue;
                case EdmType.Int32:
                    return property.Int32Value;
                case EdmType.Int64:
                    return property.Int64Value;
                default:
                    throw new Exception("not supported " + property.PropertyType);
            }
        }
    }
}
