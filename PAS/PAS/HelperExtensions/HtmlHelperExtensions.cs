using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using CPMS.Patient.Domain;

namespace PAS.HelperExtensions
{

    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString RadioButtonListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes = null) where TModel : class
        {
            var field = ExpressionHelper.GetExpressionText(expression);
            var fieldname = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(field);
            var inputName = fieldname;
            var val = GetValue(htmlHelper, expression);

            var divTag = new TagBuilder("div");
            divTag.MergeAttribute("id", inputName);
            divTag.MergeAttribute("class", "radio");
            foreach (var item in Enum.GetValues(val.GetType()))
            {
                var attr = (DisplayAttribute[])item.GetType().GetField(item.ToString()).GetCustomAttributes(typeof(DisplayAttribute), true);
                if (attr.Length != 0 && attr[0].Name == null) continue;
                var name = attr.Length > 0 && !string.IsNullOrWhiteSpace(attr[0].Name) ? attr[0].Name : item.ToString();
                var itemval = item;
                var radioButtonTag = RadioButton(htmlHelper, inputName, new SelectListItem { Text = name, Value = itemval.ToString(), Selected = val.Equals(itemval) }, htmlAttributes);

                divTag.InnerHtml += radioButtonTag;
            }

            return new MvcHtmlString(divTag.ToString());
        }

        public static MvcHtmlString DropDownListForEnum<TEnum>(
                    this HtmlHelper htmlHelper,
                    string name, string optionalLabel)
        {
            IEnumerable<TEnum> values = Enum.GetValues(typeof(TEnum))
                                        .Cast<TEnum>();
            IEnumerable<SelectListItem> items = from value in values
                                                select new SelectListItem
                                                {
                                                    Text = value.GetDescription(),
                                                    Value = value.ToString()
                                                };
            return htmlHelper.DropDownList(name, items, optionalLabel);
        }

        private static string RadioButton(this HtmlHelper htmlHelper, string name, SelectListItem listItem, IDictionary<string, object> htmlAttributes)
        {
            var inputIdSb = new StringBuilder();
            inputIdSb.Append(name)
                .Append("_")
                .Append(listItem.Value);

            var sb = new StringBuilder();

            var builder = new TagBuilder("input");
            if (listItem.Selected) builder.MergeAttribute("checked", "checked");
            builder.MergeAttribute("type", "radio");
            builder.MergeAttribute("value", listItem.Value);
            builder.MergeAttribute("id", inputIdSb.ToString());
            builder.MergeAttribute("name", name);
            builder.MergeAttributes(htmlAttributes);
            sb.Append(builder.ToString(TagRenderMode.SelfClosing));
            sb.Append(RadioButtonLabel(inputIdSb.ToString(), listItem.Text, htmlAttributes));
            sb.Append("<br>");

            return sb.ToString();
        }

        private static string RadioButtonLabel(string inputId, string displayText, IDictionary<string, object> htmlAttributes)
        {
            var labelBuilder = new TagBuilder("label");
            labelBuilder.MergeAttribute("for", inputId);
            labelBuilder.MergeAttributes(htmlAttributes);
            labelBuilder.InnerHtml = displayText;

            return labelBuilder.ToString(TagRenderMode.Normal);
        }


        private static TProperty GetValue<TModel, TProperty>(HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression) where TModel : class
        {
            var model = htmlHelper.ViewData.Model;
            if (model == null)
            {
                return default(TProperty);
            }
            var func = expression.Compile();
            return func(model);
        }
    }
}