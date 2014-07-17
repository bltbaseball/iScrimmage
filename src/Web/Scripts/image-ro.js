$(document).ready(function () {
    $("#sh_create").mouseenter(
      function () {
          $(".sh_title").text("Create Your Team In 3 Easy Steps!");
          $(".sh_subtitle").hide();
          $("#sh_image_con").css({ "background-image": "url(../images/sh_main_create.png)" });
          $("#sh_title_edge_m_lg").css({ "background-image": "url(../images/sh_bg_m_wrap_nowhite.png)" });
      }
    );
    $("#sh_join").mouseenter(
      function () {
          $(".sh_title").text("Join A Team Today!");
          $(".sh_subtitle").hide();
          $("#sh_image_con").css({ "background-image": "url(../images/sh_main_join.png)" });
          $("#sh_title_edge_m_lg").css({ "background-image": "url(../images/sh_bg_m_wrap_nowhite.png)" });
      }
    );
    $("#sh_tourn").mouseenter(
        function () {
            $(".sh_title").text("Play Now! Any Team, Any League");
            $(".sh_subtitle").show();
            $("#sh_image_con").css({ "background-image": "url(../images/sh_main_02.png)" });
            $("#sh_title_edge_m_lg").css({ "background-image": "url(../images/sh_bg_m_wrap.png)" });
        }
    );
    
});