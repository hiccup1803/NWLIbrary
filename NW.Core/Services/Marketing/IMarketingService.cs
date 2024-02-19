using NW.Core.Entities.Marketing;
using NW.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Services.Marketing
{
    public interface IMarketingService
    {
        IList<OptimoveTemplate> GetOptimoveTemplateList(int? templateType);
        void InsertOptimoveTemplate(TemplateType templateType, StatusType statusType, string name, string content);
        void DeleteOptimoveTemplate(int id);
    }
}
