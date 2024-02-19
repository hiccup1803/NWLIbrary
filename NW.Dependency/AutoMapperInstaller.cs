using AutoMapper;
using NW.Core.Entities;
using NW.Core.Entities.Payment;
using NW.Core.Model.Finance;
using NW.Core.Model.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.AutoStartInstaller
{
    public class AutoMapperInstaller
    {
        public static void Initialize()
        {
            Mapper.CreateMap<Category, CategoryModel>();
            Mapper.CreateMap<Game, GameModel>();
            //Mapper.CreateMap<GameSortable, GameModel>();
            Mapper.CreateMap<CategoryTemplate, CategoryTemplateModel>();
            Mapper.CreateMap<Bank, BankModel>();
            Mapper.CreateMap<BankTransferBankAccount, BankTransferBankAccountModel>();

        }

    }
}
