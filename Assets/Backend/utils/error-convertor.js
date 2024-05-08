export const createErrorMessage = (message) => ({error_message: message});

export const createErrorMessageWithType = (message, type) => ({type, error_message: message});