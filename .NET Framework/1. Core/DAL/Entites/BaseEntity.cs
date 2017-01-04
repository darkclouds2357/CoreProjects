using DAL.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entites
{
    public abstract class BaseEntity
    {
        public BaseEntity()
        {
            IsActive = true;
            CreatedDate = DateTime.Now;
            UpdatedDate = DateTime.Now;
            CreatedBy = string.Empty;
            UpdatedBy = string.Empty;
        }
        
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [CustomColumn("id", Order = 1)]
        public virtual long Id { get; set; }

        #region Common properties
        
        [CustomColumn("is_active")]
        [Display(Name = "Is Active")]
        public virtual bool IsActive { get; set; }
        
        [CustomColumn("created_on")]
        [Display(Name = "Created Date")]
        public virtual DateTime? CreatedDate { get; set; }
        
        [CustomColumn("updated_on")]
        [Display(Name = "Updated Date")]
        public virtual DateTime? UpdatedDate { get; set; }
        
        [MaxLength(100)]
        [CustomColumn("created_by")]
        [Display(Name = "Created By")]
        public virtual string CreatedBy { get; set; }
        
        [MaxLength(100)]
        [Display(Name = "Updated By")]
        [CustomColumn("updated_by")]
        public virtual string UpdatedBy { get; set; }

        #endregion Common properties
        
        public abstract Expression<Func<T, bool>> SetSecuredCondition<T>() where T : BaseEntity;
    }
}
