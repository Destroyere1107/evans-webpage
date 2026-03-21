// Museum-specific initialization for calculator sub-pages
// Requires SiteIncludes from ../includes.js (loaded first)
(function() {

  // Load header, fix its links, and highlight Calculator Museum nav
  SiteIncludes.loadPartial('../header.html', 'header-container', function() {
    SiteIncludes.fixRelativeLinks('#header');
    SiteIncludes.fixRelativeLinks('#navigation');
    var el = document.getElementById('nav-calculator-museum');
    if (el) el.className = 'active';
  });

  // Load footer and fix its links
  SiteIncludes.loadPartial('../footer.html', 'footer-container', function() {
    SiteIncludes.fixRelativeLinks('#footer');
  });

  // Load shared museum sidebar
  SiteIncludes.loadPartial('calc-sidebar.html', 'calc-sidebar-container');

  // Load shared license notice
  SiteIncludes.loadPartial('calc-license.html', 'calc-license-container');

  // Create theme toggle
  SiteIncludes.createThemeToggle();

})();
