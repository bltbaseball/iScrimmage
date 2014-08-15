using System;
using iScrimmage.Core.Data.Models;

namespace iScrimmage.Core.Extensions
{
    public static class BaseModelExtensions
    {
        public static BaseModel SetAuditInfoCreate(this BaseModel entity, Member user)
        {
            entity.CreatedOn = DateTime.UtcNow;
            entity.CreatedBy = user.Id;
            entity.ModifiedOn = DateTime.UtcNow;
            entity.ModifiedBy = user.Id;

            return entity;
        }

        public static BaseModel SetAuditInfoModify(this BaseModel entity, Member user)
        {
            entity.ModifiedOn = DateTime.UtcNow;
            entity.ModifiedBy = user.Id;

            return entity;
        }
    }
}
