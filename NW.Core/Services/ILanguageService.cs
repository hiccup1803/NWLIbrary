using NW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Services
{
    public interface ILanguageService
    {
        IList<Language> Languages();
        Language Language(int id);
        Language Language(string lang);
        Resource Resource(int id);
        Resource Resource(Resource resource);
        Resource Resource(string className, string culture, string resourceName);
        Resource Resource(string className, int languageId, string resourceName);
        void InsertResource(Resource resource);
        void UpdateResource(Resource resource);
        bool ResourceNameExists(Resource resource);
        bool ResourceNameExists(string className, int languageId, string resourceName);
    }
}
