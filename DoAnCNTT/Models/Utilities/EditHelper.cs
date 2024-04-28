using DoAnCNTT.Data;


namespace DoAnCNTT.Models.Utilities
{
    public static class EditHelper<T>
    {
        public static bool HasChanges(T newEntity, T existingEntity)
        {
            //Get class properties
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                if (property.Name == "ModifiedById" || property.Name == "ModifiedOn")
                {
                    continue;
                }
                if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                {
                    continue;
                }
                var newValue = property.GetValue(newEntity);
                var existingValue = property.GetValue(existingEntity);
                if (!object.Equals(newValue, existingValue))
                {
                    return true;
                }
            }
            return false;
        }   
        
        public static void SetModifiedIfNecessary(T entity, bool hasChanges, T existingEntity, string newUserId)
        {
            var modifiedByIdProperty = typeof(T).GetProperty("ModifiedById");
            if (modifiedByIdProperty != null)
                modifiedByIdProperty.SetValue(entity, hasChanges ? newUserId: (string?)modifiedByIdProperty.GetValue(existingEntity));

            var modifiedOnProperty = typeof(T).GetProperty("ModifiedOn");
            if (modifiedOnProperty != null)
                modifiedOnProperty.SetValue(entity, hasChanges ? DateTime.Now : (DateTime?)modifiedOnProperty.GetValue(existingEntity));
        }
    }
}
