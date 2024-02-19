using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NW.Core.Services.Payment;
using NW.Data.NHibernate.Map;
using NW.Data.NHibernate.Repositories;
using NW.AutoStartInstaller.Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Reflection;
using NW.Services;
using NW.Data.NHibernate.Map.Member;
using NW.Core.Work;
using NW.Data.NHibernate.Work;
using NW.Core.Repositories;
using NW.Core.Services;
using NW.Service.Localization;
using NW.Service.Cache;
using NW.Service.Messaging;
using NW.Service.ContentManagement;
using NW.Service;
using NW.Service.Tagging;
using NW.Core.Services.Marketing;
using NW.Service.Marketing;

namespace NW.AutoStartInstaller
{
    public class NWDependencyInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            //container.Kernel.ComponentRegistered += Kernel_ComponentRegistered;

            //Register all components
            container.Register(


                //Nhibernate session factory
                Component.For<ISessionFactory>().UsingFactoryMethod(CreateSessionFactory).LifestyleSingleton(),


                Component.For<IUnitOfWork>().ImplementedBy<UnitOfWork>().LifestyleSingleton(),
                Component.For<ISession>().UsingFactoryMethod(k => k.Resolve<ISessionFactory>().OpenSession()).LifeStyle.PerWebRequest,
            //Unitofwork interceptor
            //Component.For<UnitOfWorkInterceptor>().LifeStyle.Transient,

            Component.For(typeof(IRepository<,>)).ImplementedBy(typeof(Repository<,>)).LifestylePerWebRequest(),

                //All repoistories
                Classes.FromAssembly(Assembly.GetAssembly(typeof(MemberRepository))).InSameNamespaceAs<MemberRepository>().WithService.DefaultInterfaces().LifestylePerWebRequest(),
                //All repoistories
                //Classes.FromAssembly(Assembly.GetAssembly(typeof(CompanySettingRepository))).InSameNamespaceAs<CompanySettingRepository>().WithService.DefaultInterfaces().LifestylePerWebRequest(),



                Classes.FromAssemblyNamed("NW.Web.Core").BasedOn<ICepbankPaymentService>().LifestylePerWebRequest(),
                Classes.FromAssemblyNamed("NW.Web.Core").BasedOn<IPaparaPaymentService>().LifestylePerWebRequest(),
                Classes.FromAssemblyNamed("NW.Web.Core").BasedOn<IPayfixPaymentService>().LifestylePerWebRequest(),


                //All services
                //Classes.FromAssembly(Assembly.GetAssembly(typeof(MemberService))).InSameNamespaceAs<MemberService>().WithService.DefaultInterfaces().LifestylePerWebRequest()                

                Component.For(typeof(ICacheService)).ImplementedBy(typeof(CacheService)).LifestyleSingleton().IsFallback(),
                Component.For(typeof(ICategoryService)).ImplementedBy(typeof(CategoryService)).LifestylePerWebRequest().IsFallback(),
                Component.For(typeof(IContentPageService)).ImplementedBy(typeof(ContentPageService)).LifestylePerWebRequest(),
                Component.For(typeof(IFAQService)).ImplementedBy(typeof(FAQService)).LifestylePerWebRequest(),
                Component.For(typeof(IGameService)).ImplementedBy(typeof(GameService)).LifestylePerWebRequest().IsFallback(),
                Component.For(typeof(IMemberService)).ImplementedBy(typeof(MemberService)).LifestylePerWebRequest().IsFallback(),
                Component.For(typeof(ITrackService)).ImplementedBy(typeof(TrackService)).LifestylePerWebRequest(),
                Component.For(typeof(IPaymentService)).ImplementedBy(typeof(PaymentService)).LifestylePerWebRequest(),
                Component.For(typeof(ICompanyService)).ImplementedBy(typeof(CompanyService)).LifestylePerWebRequest().IsFallback(),
                Component.For(typeof(IPromotionService)).ImplementedBy(typeof(PromotionService)).LifestylePerWebRequest().IsFallback(),
                Component.For(typeof(IMailingProcessService)).ImplementedBy(typeof(MailingProcessService)).LifestylePerWebRequest().IsFallback(),
                Component.For(typeof(ILanguageService)).ImplementedBy(typeof(LanguageService)).LifestylePerWebRequest().IsFallback(),
                Component.For(typeof(ICepBankService)).ImplementedBy(typeof(NW.Service.Payment.CepBankService)).LifestylePerWebRequest(),
                Component.For(typeof(IWithdrawContainerService)).ImplementedBy(typeof(NW.Service.Payment.WithdrawContainerService)).LifestylePerWebRequest(),
                Component.For(typeof(IMemberSegmentContainerService)).ImplementedBy(typeof(NW.Service.Member.MemberSegmentContainerService)).LifestylePerWebRequest(),
                Component.For(typeof(IMessageService)).ImplementedBy(typeof(MessageService)).LifestylePerWebRequest(),
                Component.For(typeof(IMarketingBannerService)).ImplementedBy(typeof(MarketingBannerService)).LifestylePerWebRequest(),
                Component.For(typeof(IMemberSegmentService)).ImplementedBy(typeof(MemberSegmentService)).LifestylePerWebRequest(),
                Component.For(typeof(ITagService)).ImplementedBy(typeof(TagService)).LifestylePerWebRequest(),
                Component.For(typeof(IBannerService)).ImplementedBy(typeof(BannerService)).LifestylePerWebRequest(),
                Component.For(typeof(ITournamentService)).ImplementedBy(typeof(TournamentService)).LifestylePerWebRequest(),
                Component.For(typeof(IProviderService)).ImplementedBy(typeof(ProviderService)).LifestylePerWebRequest()


                );
        }

        /// <summary>
        /// Creates NHibernate Session Factory.
        /// </summary>
        /// <returns>NHibernate Session Factory</returns>
        private static ISessionFactory CreateSessionFactory()
        {
            var connStr = ConfigurationManager.ConnectionStrings["DBt"].ConnectionString;
            return Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2008.ConnectionString(connStr))
                .Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.GetAssembly(typeof(MemberMap))))
                .BuildSessionFactory();
        }

        //void Kernel_ComponentRegistered(string key, Castle.MicroKernel.IHandler handler)
        //{
        //    //Intercept all methods of all repositories.
        //    if (UnitOfWorkHelper.IsRepositoryClass(handler.ComponentModel.Implementation))
        //    {
        //        handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(UnitOfWorkInterceptor)));
        //    }

        //    //Intercept all methods of classes those have at least one method that has UnitOfWork attribute.
        //    foreach (var method in handler.ComponentModel.Implementation.GetMethods())
        //    {
        //        if (UnitOfWorkHelper.HasUnitOfWorkAttribute(method))
        //        {
        //            handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(UnitOfWorkInterceptor)));
        //            return;
        //        }
        //    }
        //}
    }
}