using MvcMiniProfiler;

namespace UCTemplate.Web.Mvc.Controllers
{
    #region using

    using System;
    using System.Linq;
    using System.Web.Mvc;

    using MvcContrib.Pagination;
    using MvcContrib.Sorting;
    using MvcContrib.UI.Grid;

    using Helpers.Attributes;
    using Infrastructure.EntityFramework;
    using Infrastructure.Mapping;
    using ViewModels;

    #endregion

    [AuthWithRedirect("~/Error/Authorization", Roles = "DEPT.its.staff")]
    public class AdminController : AbstractController
    {
        private readonly UCTemplateContext _db;

        public AdminController()
        {
            _db = new UCTemplateContext();
        }

        public ActionResult Index(string filter, string level, DateTime? from, DateTime? to, int? page, GridSortOptions sort)
        {
            var profiler = MiniProfiler.Current;

            using (profiler.Step("Query database"))
            {
                ViewBag.Level = _db.Logs.Select(x => x.Level).Distinct()
                    .Select(x => new SelectListItem {Text = x, Value = x});

                var logs = _db.Logs.AsQueryable();

                if (from.HasValue)
                {
                    logs = logs.Where(x => x.Date >= from.Value);
                }

                if (to.HasValue)
                {
                    logs = logs.Where(x => x.Date <= to.Value);
                }

                if (!string.IsNullOrWhiteSpace(filter))
                {
                    logs = logs.Where(x => x.Message.Contains(filter) || x.Exception.Contains(filter));
                }

                if (!string.IsNullOrWhiteSpace(level))
                {
                    logs = logs.Where(x => x.Level == level);
                }

                if (sort.Column != null)
                    logs = logs.OrderBy(sort.Column, sort.Direction);

                ViewBag.Sort = sort;

                using (profiler.Step("Build viewModel"))
                {
                    var viewModel = new DashboardViewModel
                                        {
                                            Logs = logs.MapTo<LogViewModel>().AsPagination(page ?? 1, 10)
                                        };
                    return View(viewModel);
                }
            }
            
        }

    }
}
