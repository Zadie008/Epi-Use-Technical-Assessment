// ========== Helper for Gravatar ==========
function getGravatarUrl(email) {
    if (!email) return "https://www.gravatar.com/avatar/?d=identicon";

    try {
        // Use the already loaded CryptoJS library
        const emailHash = CryptoJS.MD5(email.trim().toLowerCase()).toString();
        return `https://www.gravatar.com/avatar/${emailHash}?d=identicon`;
    } catch (error) {
        console.error("Error generating Gravatar URL:", error);
        return "https://www.gravatar.com/avatar/?d=identicon";
    }
}

// ========== Main D3 Hierarchy ==========
function renderD3(rootData) {
    console.log("Rendering hierarchy with data:", rootData);

    // Check if rootData is valid
    if (!rootData || !rootData.id) {
        console.error("Invalid root data:", rootData);
        alert("Invalid hierarchy data received");
        return;
    }

    const container = document.getElementById("hierarchy-container");
    if (!container) {
        console.error("Container element not found");
        return;
    }

    const width = container.clientWidth;
    const height = container.clientHeight;
    console.log("Container dimensions:", width, "x", height);

    // Clear previous visualization
    d3.select("#hierarchy-container").selectAll("*").remove();

    // Create tree layout
    const treeLayout = d3.tree()
        .size([height - 100, width - 200])  // Note: swapped for vertical layout
        .nodeSize([100, 200]);

    // Create hierarchy
    const root = d3.hierarchy(rootData);
    treeLayout(root);

    // Create SVG
    const svg = d3.select("#hierarchy-container")
        .append("svg")
        .attr("width", width)
        .attr("height", height)
        .attr("viewBox", `0 0 ${width} ${height}`);

    // Add zoom functionality
    const zoom = d3.zoom()
        .scaleExtent([0.1, 3])
        .on("zoom", function (event) {
            g.attr("transform", event.transform);
        });

    svg.call(zoom);

    const g = svg.append("g");

    // Calculate bounds for centering
    let x0 = Infinity, x1 = -Infinity, y0 = Infinity, y1 = -Infinity;
    root.each(d => {
        if (d.x > x1) x1 = d.x;
        if (d.x < x0) x0 = d.x;
        if (d.y > y1) y1 = d.y;
        if (d.y < y0) y0 = d.y;
    });

    const initialTransform = d3.zoomIdentity
        .translate(width / 2, height / 2 - (y1 - y0) / 2)
        .scale(1);

    g.attr("transform", `translate(${initialTransform.x},${initialTransform.y})`);

    // Draw links
    g.selectAll(".link")
        .data(root.links())
        .enter().append("path")
        .attr("class", "link")
        .attr("d", d3.linkVertical()
            .x(d => d.y)
            .y(d => d.x));

    // Draw nodes
    const node = g.selectAll(".node")
        .data(root.descendants())
        .enter().append("g")
        .attr("class", d => d.data.id === "org-root" ? "node root-node" : "node")
        .attr("transform", d => `translate(${d.y},${d.x})`)
        .on("click", function (event, d) {
            if (d.data.id === "org-root") return;
            console.log("Clicked on:", d.data.id);
            window.location.href = `ViewEmployee.aspx?empId=${encodeURIComponent(d.data.id)}`;
            event.preventDefault();
        });

    // Node circles
    node.append("circle")
        .attr("r", 10)
        .attr("fill", "#fff")
        .attr("stroke", "#4a90e2")
        .attr("stroke-width", 2);

    // Node text (simple version for debugging)
    node.append("text")
        .attr("dy", "0.31em")
        .attr("text-anchor", "middle")
        .text(d => d.data.Name || d.data.id)
        .attr("font-size", "12px");

    console.log("Hierarchy rendered successfully");
}

function showSuccessPanel() {
    var panel = document.getElementById('<%= pnlSuccess.ClientID %>');
    if (panel) {
        panel.style.display = 'block';
    }
}

function hideSuccessPanel() {
    var panel = document.getElementById('<%= pnlSuccess.ClientID %>');
    if (panel) {
        panel.style.display = 'none';
    }
}

function showDeletePanel() {
    var panel = document.getElementById('<%= pnlDeleteConfirm.ClientID %>');
    if (panel) {
        panel.style.display = 'block';
    }
}

function hideDeletePanel() {
    var panel = document.getElementById('<%= pnlDeleteConfirm.ClientID %>');
    if (panel) {
        panel.style.display = 'none';
    }
}

// Close panels when clicking outside
document.addEventListener('click', function (e) {
    // Success Panel
    var successPanel = document.getElementById('<%= pnlSuccess.ClientID %>');
    if (successPanel && successPanel.style.display === 'block') {
        var successContent = successPanel.querySelector('.success-panel-content');
        if (successContent && !successContent.contains(e.target)) {
            hideSuccessPanel();
        }
    }

    // Delete Panel
    var deletePanel = document.getElementById('<%= pnlDeleteConfirm.ClientID %>');
    if (deletePanel && deletePanel.style.display === 'block') {
        var deleteContent = deletePanel.querySelector('.modal-panel-content');
        if (deleteContent && !deleteContent.contains(e.target)) {
            hideDeletePanel();
        }
    }
});

// Auto-close success panel after 5 seconds
function autoCloseSuccessPanel() {
    setTimeout(function () {
        hideSuccessPanel();
    }, 5000);
}

// Re-bind events after AJAX update
var prm = Sys.WebForms.PageRequestManager.getInstance();
prm.add_endRequest(function () {
    // Re-bind any JavaScript events here if needed
    bindTabEvents();
});

function bindTabEvents() {
    $('.tab-button').off('click').on('click', function (e) {
        e.preventDefault();
        $('.tab-button').removeClass('active');
        $(this).addClass('active');
        $('.tab-content').hide();
        var tabName = $(this).attr('data-tab');
        $('#' + tabName).show();
        $('#<%= activeTabHidden.ClientID %>').val(tabName);
    });
}