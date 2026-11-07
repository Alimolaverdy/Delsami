$(window).on('load', function () {

    var element = $('.home-page-blog-social-wrapper .post-list').height()
    var elementWrapper = $('.home-page-blog-social-wrapper .post-list-wrap').height()

    var elementScroll = document.querySelector('.home-page-blog-social-wrapper .post-list-wrap');

    if (element > elementWrapper) {
        SimpleScrollbar.initEl(elementScroll);
    }


    if ($('.pinterest-feed .post-list').length > 0) {
        $('.pinterest-feed .post-list').masonry({
            itemSelector: '.post-item',
            percentPosition: true
        });
    }

});