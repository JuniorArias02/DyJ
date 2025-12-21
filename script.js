// ===== GLOBAL VARIABLES =====
const uploadZone = document.getElementById('uploadZone');
const fileInput = document.getElementById('fileInput');
const previewArea = document.getElementById('previewArea');
const downloadBtn = document.getElementById('downloadBtn');

let uploadedFiles = [];

// ===== EVENT LISTENERS =====

// Click to upload
uploadZone.addEventListener('click', () => {
    fileInput.click();
});

// File input change
fileInput.addEventListener('change', (e) => {
    handleFiles(e.target.files);
});

// Drag and drop events
uploadZone.addEventListener('dragover', (e) => {
    e.preventDefault();
    uploadZone.classList.add('drag-over');
});

uploadZone.addEventListener('dragleave', () => {
    uploadZone.classList.remove('drag-over');
});

uploadZone.addEventListener('drop', (e) => {
    e.preventDefault();
    uploadZone.classList.remove('drag-over');
    handleFiles(e.dataTransfer.files);
});

// Download button
downloadBtn.addEventListener('click', downloadAllImages);

// ===== MAIN FUNCTIONS =====

/**
 * Handle uploaded files
 * @param {FileList} files - Files to process
 */
function handleFiles(files) {
    const pngFiles = Array.from(files).filter(file => file.type === 'image/png');
    
    if (pngFiles.length === 0) {
        showNotification('Por favor, sube solo archivos PNG', 'error');
        return;
    }
    
    pngFiles.forEach((file, index) => {
        processFile(file, index);
    });
    
    // Show download button if there are files
    if (uploadedFiles.length > 0) {
        downloadBtn.classList.remove('hidden');
    }
}

/**
 * Process individual file
 * @param {File} file - File to process
 * @param {number} index - Index for naming
 */
function processFile(file, index) {
    const reader = new FileReader();
    
    reader.onload = (e) => {
        const img = new Image();
        img.onload = () => {
            // Rename and save file
            const renamedFile = renameFile(file);
            uploadedFiles.push(renamedFile);
            
            // Create preview
            createPreview(e.result, renamedFile);
        };
        img.src = e.target.result;
    };
    
    reader.readAsDataURL(file);
}

/**
 * Rename file to DaniYJuni.png (with number if multiple)
 * @param {File} file - Original file
 * @returns {File} - Renamed file
 */
function renameFile(file) {
    const existingCount = uploadedFiles.filter(f => 
        f.name.startsWith('DaniYJuni')
    ).length;
    
    const newName = existingCount === 0 
        ? 'DaniYJuni.png' 
        : `DaniYJuni_${existingCount + 1}.png`;
    
    return new File([file], newName, { type: file.type });
}

/**
 * Create preview element
 * @param {string} src - Image source
 * @param {File} file - File object
 */
function createPreview(src, file) {
    const previewItem = document.createElement('div');
    previewItem.className = 'preview-item';
    
    const fileSize = formatFileSize(file.size);
    
    previewItem.innerHTML = `
        <img src="${src}" alt="${file.name}" class="preview-image">
        <div class="preview-info">
            <div class="preview-name">${file.name}</div>
            <div class="preview-size">${fileSize}</div>
            <div class="preview-status">
                <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path d="M20 6L9 17L4 12" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                </svg>
                Listo
            </div>
        </div>
    `;
    
    previewArea.appendChild(previewItem);
}

/**
 * Download all images as a zip (simulated by downloading individually)
 */
function downloadAllImages() {
    if (uploadedFiles.length === 0) {
        showNotification('No hay imágenes para descargar', 'error');
        return;
    }
    
    uploadedFiles.forEach((file, index) => {
        setTimeout(() => {
            downloadFile(file);
        }, index * 200); // Stagger downloads
    });
    
    showNotification(`Descargando ${uploadedFiles.length} imagen(es)...`, 'success');
}

/**
 * Download individual file
 * @param {File} file - File to download
 */
function downloadFile(file) {
    const url = URL.createObjectURL(file);
    const a = document.createElement('a');
    a.href = url;
    a.download = file.name;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
}

/**
 * Format file size to human readable
 * @param {number} bytes - File size in bytes
 * @returns {string} - Formatted size
 */
function formatFileSize(bytes) {
    if (bytes === 0) return '0 Bytes';
    
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    
    return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
}

/**
 * Show notification (simple implementation)
 * @param {string} message - Message to show
 * @param {string} type - Type of notification (success, error)
 */
function showNotification(message, type) {
    // Create notification element
    const notification = document.createElement('div');
    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        padding: 1rem 1.5rem;
        background: ${type === 'success' ? 'linear-gradient(135deg, #10b981, #059669)' : 'linear-gradient(135deg, #ef4444, #dc2626)'};
        color: white;
        border-radius: 12px;
        box-shadow: 0 8px 20px rgba(0, 0, 0, 0.3);
        font-weight: 500;
        z-index: 1000;
        animation: slideIn 0.3s ease-out;
    `;
    notification.textContent = message;
    
    document.body.appendChild(notification);
    
    // Remove after 3 seconds
    setTimeout(() => {
        notification.style.animation = 'slideOut 0.3s ease-out';
        setTimeout(() => {
            document.body.removeChild(notification);
        }, 300);
    }, 3000);
}

// Add animations for notifications
const style = document.createElement('style');
style.textContent = `
    @keyframes slideIn {
        from {
            transform: translateX(400px);
            opacity: 0;
        }
        to {
            transform: translateX(0);
            opacity: 1;
        }
    }
    
    @keyframes slideOut {
        from {
            transform: translateX(0);
            opacity: 1;
        }
        to {
            transform: translateX(400px);
            opacity: 0;
        }
    }
`;
document.head.appendChild(style);
