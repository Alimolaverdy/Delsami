$(window).on('load', function () {

    //add slick slider to a twitter feed
    $('.body-ltr .twitter-feed .post-list').each(function () {
        $(this).slick({
            dots: false,
            mobileFirst: true,
            adaptiveHeight: true
        });
    });

    //add slick slider to a twitter feed RTL 
    $('.body-rtl .twitter-feed .post-list').each(function () {
        $(this).slick({
            dots: false,
            mobileFirst: true,
            adaptiveHeight: true,
            rtl: true
        });
    });

    //add masonry js and simple scrollbar to pinterest feed
    if ($('.pinterest-feed .post-list').length > 0) {
        var pinterestFeedList = document.querySelector('.pinterest-feed .post-list-wrap');
        SimpleScrollbar.initEl(pinterestFeedList);

        $('.pinterest-feed .post-list').masonry({
            itemSelector: '.post-item',
            percentPosition: true
        });
    }

    //add masonry js and simple scrollbar to google feed
    if ($('.google-feed .post-list').length > 0) {
        var googleFeedList = document.querySelector('.google-feed .post-list-wrap');
        SimpleScrollbar.initEl(googleFeedList);

        $('.google-feed .post-list').masonry({
            itemSelector: '.post-item',
            percentPosition: true
        });
    }

    //add masonry js and simple scrollbar to facebook feed
    if ($('.facebook-feed .post-list').length > 0) {
        var facebookFeedList = document.querySelector('.facebook-feed .post-list-wrap');
        SimpleScrollbar.initEl(facebookFeedList);
        
        if ($('.facebook-feed').parent().children().length == 1) {
            $('.facebook-feed .post-list').masonry({
                itemSelector: '.post-item',
                percentPosition: true
            });
        }
    }

});