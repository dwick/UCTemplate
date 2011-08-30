namespace UCTemplate.Web.Mvc.Helpers.Html
{
    #region using

    using System;
    using System.Collections.Generic;

    using MvcContrib.Sorting;
    using MvcContrib.UI.Grid;

    #endregion

    public class BootstrapTableRenderer<T> : HtmlTableGridRenderer<T> where T : class
    {
        protected override void RenderHeaderCellStart(GridColumn<T> column)
        {
            var attributes = new Dictionary<string, object>(column.HeaderAttributes);

            if (IsSortingEnabled && column.Sortable)
            {
                var sortColumnName = GenerateSortColumnName(column);

                var isSortedByThisColumn = GridModel.SortOptions.Column != null && GridModel.SortOptions.Column.Equals(sortColumnName, StringComparison.InvariantCultureIgnoreCase);

                if (isSortedByThisColumn)
                {
                    var sortClass = GridModel.SortOptions.Direction == SortDirection.Ascending ? "headerSortDown" : "headerSortUp";

                    if (attributes.ContainsKey("class") && attributes["class"] != null)
                    {
                        sortClass = string.Join(" ", new[] { attributes["class"].ToString(), sortClass });
                    }

                    attributes["class"] = sortClass;
                }
            }

            var attrs = BuildHtmlAttributes(attributes);

            if (attrs.Length > 0)
                attrs = " " + attrs;

            RenderText(string.Format("<th{0}>", attrs));
        }

        protected override void RenderRowStart(GridRowViewData<T> rowData)
        {
            var attributes = GridModel.Sections.Row.Attributes(rowData);

            if (!attributes.ContainsKey("class"))
            {
                attributes["class"] = rowData.IsAlternate ? "odd" : "even";
            }

            var attributeString = BuildHtmlAttributes(attributes);

            if (attributeString.Length > 0)
            {
                attributeString = " " + attributeString;
            }

            RenderText(string.Format("<tr{0}>", attributeString));
        }
    }
}