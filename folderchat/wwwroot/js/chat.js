window.chatFunctions = {
    scrollToBottom: function (elementId) {
        const element = document.getElementById(elementId);
        if (element) {
            element.scrollTo({
                top: element.scrollHeight,
                behavior: 'smooth'
            });
        }
    },

    scrollToBottomInstant: function (elementId) {
        const element = document.getElementById(elementId);
        if (element) {
            element.scrollTop = element.scrollHeight;
        }
    },

    isUserScrolledUp: function (elementId) {
        const element = document.getElementById(elementId);
        if (element) {
            // Check if user has scrolled up more than 50 pixels from the bottom
            const threshold = 50;
            const isScrolledUp = (element.scrollHeight - element.scrollTop - element.clientHeight) > threshold;
            return isScrolledUp;
        }
        return false;
    }
};