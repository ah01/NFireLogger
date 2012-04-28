
// fix nav on scroll
var $win = $(window)
    , $nav = $('.navbar')
    , navTop = $nav.length && $nav.offset().top - 40
    , isFixed = 0;

processScroll();

// hack sad times - holdover until rewrite for 2.1
$nav.on('click', function () {
    if (!isFixed) setTimeout(function () { $win.scrollTop($win.scrollTop() - 47); }, 10);
});

$win.on('scroll', processScroll);

function processScroll() {
    var scrollTop = $win.scrollTop();
    if (scrollTop >= navTop && !isFixed) {
        isFixed = 1;
        $nav.addClass('navbar-fixed-top')
    } else if (scrollTop <= navTop && isFixed) {
        isFixed = 0;
        $nav.removeClass('navbar-fixed-top');
    }
}


prettyPrint();

$("a.btn[data-action]")
    .click(function() {

        var action = $(this).attr("data-action");
        $.get("/home/" + action);

    })
    .popover({
        placement: "left",
        title: "Try this in your FireLogger!",
        content: "Open your FireLogger console and hit the button. Ajax request that does this logging on server will be send."
    });
