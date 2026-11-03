$(window).on('load', function () {

    //fix for the old IE 
    if (($('.social-feed').attr('data-ieoldversion') === 'True') && ($('.social-feed').length > 0)) {
        
        $('.social-feed').addClass("old-ie");
        var rtlvalue = $('.social-feed').attr("data-rtl") === 'True';

        $('.center-1 .old-ie .post-list, .center-2 .old-ie .post-list').masonry({
            itemselector: '.post-item',
            percentposition: true,
            originleft: !rtlvalue
        });
    }
    //end

    var rtlValue = $('#rtlEnabled').val() === 'True';
    var elementWrapper = $('.home-page .post-list-wrap').height();

    //add simple scrollbar to pinterest feed
    if ($('.pinterest-feed .post-list').length > 0) {
        if ($('.pinterest-feed .post-list').height() > elementWrapper) {
            var pinterestFeedList = document.querySelector('.pinterest-feed .post-list-wrap');
            SimpleScrollbar.initEl(pinterestFeedList);
        }
        
    }
    //add simple scrollbar to google feed
    if ($('.google-feed .post-list').length > 0) {
        if ($('.google-feed .post-list').height() > elementWrapper) {
            var googleFeedList = document.querySelector('.google-feed .post-list-wrap');
            SimpleScrollbar.initEl(googleFeedList);
        }
    }

    //add simple scrollbar to facebook feed
    if ($('.facebook-feed .post-list').length > 0) {
        if ($('.facebook-feed .post-list').height() > elementWrapper) {
            var facebookFeedList = document.querySelector('.facebook-feed .post-list-wrap');
            SimpleScrollbar.initEl(facebookFeedList);
        }
    }

    //add simple scrollbar to twitter feed
    if ($('.twitter-feed .post-list').length > 0) {
        if ($('.twitter-feed .post-list').height() > elementWrapper) {
            var facebookFeedList = document.querySelector('.twitter-feed .post-list-wrap');
            SimpleScrollbar.initEl(facebookFeedList);
        }
    }

    //add simple scrollbar to instegram feed
    if ($('.instagram-feed .post-list').length > 0) {
        if ($('.instagram-feed .post-list').height() > elementWrapper) {
            var facebookFeedList = document.querySelector('.instagram-feed .post-list-wrap');
            SimpleScrollbar.initEl(facebookFeedList);
        }
    }

    
    
});