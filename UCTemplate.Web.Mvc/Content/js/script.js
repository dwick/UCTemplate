/// <reference path="libs/jquery-1.6.2-vsdoc.js" />
/* Author: 
    David Wick
    wick.d.spencer@gmail.com
*/
$(function () {
    $("ul.sf-menu").supersubs().superfish();
    $('#password-policy').find('ul').hide();
    $('#password-policy a').click(function () {
        var that = this;
        $(that).siblings('ul').toggle();
        $(that).toggleClass('more less');
        $(that).html($(that).attr('class'));
    });
    $('a.close').click(function () {
        $(this).parent().remove();
    });
    Cufon.replace($('#uc-head-titles').show().find('h1,h2')); // prevent fouc
    Cufon.now();
});