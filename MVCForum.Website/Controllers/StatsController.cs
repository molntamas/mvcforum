using System;
using System.Linq;
using System.Web.Mvc;
using MVCForum.Domain.Constants;
using MVCForum.Domain.Interfaces.Services;
using MVCForum.Domain.Interfaces.UnitOfWork;
using MVCForum.Website.ViewModels;

namespace MVCForum.Website.Controllers
{
    public partial class StatsController : BaseController
    {
        private readonly ITopicService _topicService;
        private readonly IPostService _postService;

        public StatsController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, 
            ILocalizationService localizationService, IRoleService roleService, ISettingsService settingsService, ITopicService topicService, IPostService postService) : 
            base(loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService)
        {
            _topicService = topicService;
            _postService = postService;
        }

        [ChildActionOnly]
        [OutputCache(Duration = AppConstants.DefaultCacheLengthInSeconds)]
        public PartialViewResult GetMainStats()
        {
            var viewModel = new MainStatsViewModel
                                {
                                    MemberCount = MembershipService.MemberCount(),
                                    TopicCount = _topicService.TopicCount(),
                                    PostCount = _postService.PostCount()
                                };

            viewModel.LatestMembers = new System.Collections.Generic.List<System.Tuple<string, string>>();
            foreach (var user in MembershipService.GetLatestUsers(10))
            {
                if (viewModel.LatestMembers.All(u => u.Item1 != user.UserName))
                {
                    viewModel.LatestMembers.Add(new Tuple<string, string>(user.UserName,user.NiceUrl));
                }
            }

            return PartialView(viewModel);
        }
    }
}
