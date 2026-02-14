// Custom Modal Functions
function openModal() {
    var overlay = document.getElementById('member-modal-overlay');
    if (overlay) {
        overlay.style.display = 'flex';
        document.body.style.overflow = 'hidden'; // Prevent background scrolling
    }
}

function closeModal() {
    var overlay = document.getElementById('member-modal-overlay');
    if (overlay) {
        overlay.style.display = 'none';
        document.body.style.overflow = ''; // Restore background scrolling
    }
}

// Close modal on successful form submission (htmx event)
document.addEventListener('htmx:afterOnLoad', function(event) {
    // If the request was to /members (POST or PUT) and succeeded, close the modal
    if (event.detail.xhr && event.detail.xhr.status === 200) {
        var url = event.detail.pathInfo.requestPath;
        if (url.startsWith('/members') && (event.detail.verb === 'POST' || event.detail.verb === 'PUT')) {
            closeModal();
        }
    }

    // Update attendance counter after attendance list loads
    if (event.detail.xhr && event.detail.pathInfo.requestPath === '/attendance/list') {
        updateAttendanceCount();
    }

    // Reset member filters after members table is swapped (add/edit/deactivate)
    if (event.detail.xhr && event.detail.pathInfo.requestPath.startsWith('/members')) {
        var ageSelect = document.getElementById('member-age-group-filter');
        var nameInput = document.getElementById('member-name-filter');
        if (ageSelect) ageSelect.value = '';
        if (nameInput) nameInput.value = '';
    }
});

// Update attendance counter display (compact checklist version)
function updateAttendanceCount() {
    var topBarCount = document.getElementById('top-bar-count');
    if (!topBarCount) return;

    var totalChecked = 0;

    // Update per-section counts
    var sections = document.querySelectorAll('.age-group-section');
    sections.forEach(function(section) {
        var checkboxes = section.querySelectorAll('input[name="memberIds"]');
        var checked = 0;
        checkboxes.forEach(function(cb) { if (cb.checked) checked++; });
        totalChecked += checked;

        var countSpan = section.querySelector('.section-checked');
        if (countSpan) countSpan.textContent = checked;
    });

    // Update top bar total
    topBarCount.textContent = totalChecked + ' present';
}

// Apply name and age group filters on the members page
function filterMemberRows() {
    var ageSelect = document.getElementById('member-age-group-filter');
    var nameInput = document.getElementById('member-name-filter');
    var ageValue = ageSelect ? ageSelect.value : '';
    var nameValue = nameInput ? nameInput.value.toLowerCase() : '';
    var rows = document.querySelectorAll('#members-table tr[data-age-group]');
    rows.forEach(function (row) {
        var matchesAge = !ageValue || row.getAttribute('data-age-group') === ageValue;
        var matchesName = !nameValue || row.getAttribute('data-name').indexOf(nameValue) !== -1;
        row.style.display = (matchesAge && matchesName) ? '' : 'none';
    });
}

// Filter attendance rows by name (compact checklist version)
function filterAttendanceRows() {
    var nameInput = document.getElementById('name-filter');
    var nameValue = nameInput ? nameInput.value.toLowerCase() : '';

    var sections = document.querySelectorAll('.age-group-section');
    sections.forEach(function(section) {
        var rows = section.querySelectorAll('.attendance-row');
        var visibleCount = 0;
        rows.forEach(function(row) {
            var matchesName = !nameValue || row.getAttribute('data-name').indexOf(nameValue) !== -1;
            row.style.display = matchesName ? '' : 'none';
            if (matchesName) visibleCount++;
        });
        // Hide entire section if no visible members
        section.style.display = visibleCount > 0 ? '' : 'none';
    });
}

// Auto-save attendance on checkbox change
function autoSaveAttendance() {
    var form = document.querySelector('form[hx-post="/attendance"]');
    if (!form) return;
    var formData = new FormData(form);
    var status = document.getElementById('auto-save-status');
    if (status) status.innerHTML = '<em>Saving...</em>';
    fetch('/attendance/auto-save', { method: 'POST', body: formData })
        .then(function (r) { return r.text(); })
        .then(function (html) { if (status) status.innerHTML = html; })
        .catch(function () { if (status) status.innerHTML = '<span class="status-msg error" style="padding:0.3rem 0.6rem;font-size:0.85rem">Save failed</span>'; });
}

// Attendance and member page change events
document.addEventListener('change', function (e) {
    // Auto-save on individual checkbox toggle
    if (e.target && e.target.name === 'memberIds') {
        updateAttendanceCount();
        autoSaveAttendance();
    }

    // Age group filter (members page)
    if (e.target && e.target.id === 'member-age-group-filter') {
        filterMemberRows();
    }
});

// Name filter (fires on every keystroke)
document.addEventListener('input', function (e) {
    if (e.target && e.target.id === 'name-filter') {
        filterAttendanceRows();
    }

    // Name filter (members page)
    if (e.target && e.target.id === 'member-name-filter') {
        filterMemberRows();
    }

    // Sync report date inputs with hidden form fields
    if (e.target.id === 'startDate') {
        document.querySelectorAll('.report-start-date').forEach(function (el) {
            el.value = e.target.value;
        });
    }
    if (e.target.id === 'endDate') {
        document.querySelectorAll('.report-end-date').forEach(function (el) {
            el.value = e.target.value;
        });
    }
});

// PDF Sharing functionality
function getPdfFileName() {
    var startDate = document.getElementById('startDate').value;
    var endDate = document.getElementById('endDate').value;
    return 'Church-Attendance-Report-' + startDate + '-to-' + endDate + '.pdf';
}

function sharePdf() {
    var statusDiv = document.getElementById('report-status');

    // Check if Web Share API is supported
    if (!navigator.share || !navigator.canShare) {
        statusDiv.innerHTML = '<p class="status-msg error">Sharing is not supported on this device. Use "Download PDF" instead.</p>';
        return;
    }

    statusDiv.innerHTML = '<p class="status-msg">Preparing PDF for sharing...</p>';

    var fileName = getPdfFileName();
    var startDate = document.getElementById('startDate').value;
    var endDate = document.getElementById('endDate').value;

    // Fetch the PDF as a blob
    var formData = new FormData();
    formData.append('startDate', startDate);
    formData.append('endDate', endDate);

    fetch('/reports/export', {
        method: 'POST',
        body: formData
    })
    .then(function(response) {
        if (!response.ok) {
            throw new Error('Failed to generate PDF');
        }
        return response.blob();
    })
    .then(function(blob) {
        // Create a File object from the blob
        var file = new File([blob], fileName, { type: 'application/pdf' });

        // Check if we can share files
        if (navigator.canShare && navigator.canShare({ files: [file] })) {
            return navigator.share({
                files: [file],
                title: 'Church Attendance Report',
                text: 'Attendance report from ' + startDate + ' to ' + endDate
            });
        } else {
            // Fall back to sharing URL if file sharing isn't supported
            throw new Error('File sharing not supported');
        }
    })
    .then(function() {
        statusDiv.innerHTML = '<p class="status-msg success">PDF shared successfully!</p>';
    })
    .catch(function(error) {
        if (error.name === 'AbortError') {
            // User cancelled the share
            statusDiv.innerHTML = '';
        } else {
            // Fall back to download
            statusDiv.innerHTML = '<p class="status-msg error">Sharing failed. Downloading instead...</p>';
            document.getElementById('export-pdf-form').submit();
        }
    });
}

// PDF Sharing functionality for Attendance tab
function getAttendancePdfFileName() {
    var form = document.querySelector('form[hx-post="/attendance"]');
    if (!form) return 'Church-Attendance.pdf';

    var dateInput = form.querySelector('input[name="date"]');
    var serviceTypeInput = form.querySelector('input[name="serviceType"]');

    var date = dateInput ? dateInput.value : '';
    var serviceType = serviceTypeInput ? serviceTypeInput.value : 'SundayService';
    var serviceLabel = serviceType === 'PrayerMeeting' ? 'Prayer-Meeting' : 'Sunday-Service';

    return 'Church-Attendance-' + serviceLabel + '-' + date + '.pdf';
}

function shareAttendancePdf() {
    var statusDiv = document.getElementById('attendance-pdf-status');
    if (!statusDiv) {
        statusDiv = document.createElement('div');
        statusDiv.id = 'attendance-pdf-status';
        var formParent = document.querySelector('form[hx-post="/attendance"]');
        if (formParent) {
            formParent.appendChild(statusDiv);
        }
    }

    // Check if Web Share API is supported
    if (!navigator.share || !navigator.canShare) {
        statusDiv.innerHTML = '<p class="status-msg error">Sharing is not supported on this device. Use "View Summary" to see the report.</p>';
        return;
    }

    statusDiv.innerHTML = '<p class="status-msg">Preparing PDF for sharing...</p>';

    var fileName = getAttendancePdfFileName();
    var form = document.querySelector('form[hx-post="/attendance"]');
    if (!form) {
        statusDiv.innerHTML = '<p class="status-msg error">Form not found.</p>';
        return;
    }

    var dateInput = form.querySelector('input[name="date"]');
    var serviceTypeInput = form.querySelector('input[name="serviceType"]');

    var date = dateInput ? dateInput.value : '';
    var serviceType = serviceTypeInput ? serviceTypeInput.value : 'SundayService';
    var serviceLabel = serviceType === 'PrayerMeeting' ? 'Prayer Meeting' : 'Sunday Service';

    // Fetch the PDF as a blob
    var formData = new FormData();
    formData.append('date', date);
    formData.append('serviceType', serviceType);

    fetch('/attendance/export-pdf', {
        method: 'POST',
        body: formData
    })
    .then(function(response) {
        if (!response.ok) {
            throw new Error('Failed to generate PDF');
        }
        return response.blob();
    })
    .then(function(blob) {
        // Create a File object from the blob
        var file = new File([blob], fileName, { type: 'application/pdf' });

        // Check if we can share files
        if (navigator.canShare && navigator.canShare({ files: [file] })) {
            return navigator.share({
                files: [file],
                title: 'Church Attendance - ' + serviceLabel,
                text: 'Attendance for ' + serviceLabel + ' on ' + date
            });
        } else {
            // Fall back to download if file sharing isn't supported
            throw new Error('File sharing not supported');
        }
    })
    .then(function() {
        statusDiv.innerHTML = '<p class="status-msg success">PDF shared successfully!</p>';
    })
    .catch(function(error) {
        if (error.name === 'AbortError') {
            // User cancelled the share
            statusDiv.innerHTML = '';
        } else {
            // Fall back to download
            statusDiv.innerHTML = '<p class="status-msg error">Sharing failed. Try using "View Summary" instead.</p>';
        }
    });
}

// Intercept Export PDF button to use share API on mobile if available
document.addEventListener('DOMContentLoaded', function() {
    var exportBtn = document.getElementById('export-pdf-btn');
    var exportForm = document.getElementById('export-pdf-form');
    var shareBtn = document.getElementById('share-pdf-btn');

    if (exportBtn && exportForm) {
        exportBtn.addEventListener('click', function(e) {
            // Check if we're on a mobile device and Web Share API is available
            var isMobile = /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);

            if (isMobile && navigator.share && navigator.canShare) {
                e.preventDefault();
                sharePdf();
            }
            // Otherwise, let the form submit normally for download
        });
    }

    // Share button is always visible - sharePdf() will handle unsupported devices
});

// Collapsible age group sections
document.addEventListener('click', function(e) {
    var header = e.target.closest('.age-group-header');
    if (header) {
        var section = header.closest('.age-group-section');
        if (section) section.classList.toggle('collapsed');
    }
});

// Search toggle for compact attendance checklist
document.addEventListener('click', function(e) {
    if (e.target && (e.target.id === 'search-toggle' || e.target.closest('#search-toggle'))) {
        var topBar = document.getElementById('attendance-top-bar');
        if (topBar) {
            topBar.classList.add('searching');
            var input = document.getElementById('name-filter');
            if (input) input.focus();
        }
    }
    if (e.target && (e.target.id === 'search-close' || e.target.closest('#search-close'))) {
        var topBar = document.getElementById('attendance-top-bar');
        if (topBar) {
            topBar.classList.remove('searching');
            var input = document.getElementById('name-filter');
            if (input) {
                input.value = '';
                filterAttendanceRows();
            }
        }
    }
});
