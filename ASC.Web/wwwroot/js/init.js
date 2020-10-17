(function($){
  $(function(){

    $('.sidenav').sidenav();
    $('.parallax').parallax();

     if (window.history && window.history.pushState) {
          window.history.pushState('forward', '', window.location.href);

          $(window).on('popstate', function (e) {
              window.history.pushState('forward', '', window.location.href);
              e.preventDefault();
          });
      }

      $(document).ready(function () {
          //$(window).on("contextmenu", function () {
              //return false;
          //});

          $('select').formSelect();
      });

  }); // end of document ready
})(jQuery); // end of jQuery name space
