namespace UCTemplate.Web.Mvc.ViewModels
{
    #region using

    using System;
    using System.ComponentModel.DataAnnotations;

    using MvcContrib.Pagination;

    #endregion

    public class DashboardViewModel
    {
        [UIHint("AjaxTextbox")]
        [Display(Name = "Search", Description = "Search for messages and exceptions.")]
        public string Filter { get; set; }

        [UIHint("FilterList")]
        [Display(Name = "Level")]
        public string Level { get; set; }

        [UIHint("AjaxTextbox")]
        [Display(Name = "From")]
        public DateTime? From { get; set; }

        [UIHint("AjaxTextbox")]
        [Display(Name = "To")]
        public DateTime? To { get; set; }

        public IPagination<LogViewModel> Logs { get; set; }
    }
}