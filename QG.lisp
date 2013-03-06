(defpackage :john.QG)
(in-package :john.QG)
(defvar *texts* (make-array 5))
(defvar *blrequest* "getNumBackLinks?")
(defvar *randomrequest* "randomTest?")
;;text clean up

(defmacro while (test &body body)
  `(do ()
       ((not ,test))
     ,@body))

(defmacro open-txt (file-name)
  (let ((in (gensym)))
    `(with-open-file (,in ,file-name)
      (read-line ,in))))

(defmacro replace-element (sequence index replace)
  `(append (subseq ,sequence 0 ,index) ,replace (subseq ,sequence (+ ,index 1))))

(let ((x 0))
  (while (< x 5)
    (setf (elt *texts* x) (open-txt (concatenate 'string "~/Lisp/QG/QGTests/wiki" (write-to-string (+ 1 x)) ".txt")))
    (incf x)))

(defun trim (text)
  (reverse (trim-front (reverse (trim-front text)))))

(defun trim-front (text)
  (let ((letter #\Space)
	(begin 0)
	(end 0)
	(x t))
    (while x
      (while (or (eql letter #\Space) (eql letter #\Newline))
	(setq letter (coerce (subseq text begin (+ 1 begin)) 'character))
	(incf begin))
      (setq end begin)
      (while (and (null (eql letter #\Space)) (null(eql end (length text))))
	(setq letter (coerce (subseq text end (+ 1 end)) 'character))
	(incf end))
      (if (or (equalp (subseq text (- begin 1) (- end 1)) "(href)") (equalp (subseq text (- begin 1) (- end 1)) ")ferh("))
	  (setq begin end)
	  (setq x nil))
      (subseq text (- begin 1)))))

(defun extract-words (text)
  (let ((test 't)
	(begin 0)
	(word "")
	(words 'nil))
    (while test
      (if (/= begin 0)
	  (setq word (get-next-word text (- begin 1)))
	  (setq word (get-next-word text begin)))
      (setq begin (+ begin (length word) 1))
      (if word
      	(push word words))
      (setq test word))
    (setq words (reverse words))
    (or (elim-references words)  words)))

(defun get-next-word (text begin)
  (if (null (>= begin (length text)))
      (let ((y 0)
	    (letter (coerce (subseq text begin (+ 1 begin)) 'character)))
	(when (and (null (eql begin 0)) (null (eql letter #\Space)))
	  (while (and (null (eql letter #\Space)) (null (eql begin (length text))))
	    (setq letter (coerce (subseq text begin (+ begin 1)) 'character))
	    (incf begin)))
	(if (null (>= begin (length text) 1))
	    (progn (while  (and (eql letter #\Space) (null (eql begin (length text))))
		     (setq letter (coerce (subseq text begin (+ begin 1)) 'character))
		     (incf begin))
		   (setq y begin)
		   (while (and (null (eql letter #\Space)) (null (eql y (length text))))
		     (setq letter (coerce (subseq text y (+ y 1)) 'character))
		     (incf y))
		   (cond ((eql begin 0)(subseq text begin (- y 1)))
			 ((eql y (length text)) (subseq text (- begin 1)))
			 ((eql begin (length text)) 'nil)
			 (t (subseq text (- begin 1) (- y 1)))))))))

(defun elim-references (text)
  (if (or (find "References(Noun)" text :test #'equalp) (find "Sources(Noun)" text :test #'equalp))
      (or (subseq text 0 (position "References(Noun)" text :test #'equalp)) (subseq text 0 (position "Sources(Noun)" text :test #'equalp)))))

(defun seperate-defs (text-list)
      (let ((crrnt (cadr text-list))
	    (rst (cddr text-list))
	    (ret nil))
	(while crrnt
	  (let ((begin 0)
		(letter nil))
	    (while (and (null (eql letter #\( )) (null (eql begin (length crrnt))))
	      (setq letter (coerce (subseq crrnt begin (+ begin 1)) 'character))
	      (incf begin))
	    (if (= begin (length crrnt))
		(push (cons crrnt "()") ret)
		(push (cons (subseq crrnt 0 (- begin 1)) (subseq crrnt (- begin 1))) ret)))
	    (setq crrnt (car rst))
	    (setq rst (cdr rst)))
	(setq ret (reverse ret))
	(push (car text-list) ret)))

(defun find-href (text)
  (position (cons "" "(href)") text :test #'equalp))

(defun merge-hrefs (text-assoc-list)
      (let ((pos (find-href text-assoc-list))
	    (wrd nil)
	    (form nil))
	(while pos
	  (setq wrd (nth (+ pos 1) text-assoc-list))
	  (setq form (cons "(href)" (cons (car wrd) (cdr wrd))))
	  (setq text-assoc-list (append (subseq text-assoc-list 0 pos) (list form) (subseq text-assoc-list (+ pos 2))))
	  (setq pos (find-href text-assoc-list))))
      text-assoc-list)

(defun seperate-title (text)
  (let ((letter nil)
	(end 0)
	(ret nil))
    (while (null (eql letter #\]))
      (setq letter (coerce (subseq text end (+ end 1)) 'character))
      (incf end))
    (setq ret (cons (subseq text 0 (+ end 1)) (subseq text (+ end 1))))
    ret))
	
(defun clean-text (text)
  (let ((full-text (seperate-title text)))
    (setq text (extract-words (cdr full-text)))
    (push (car full-text) text))
  (substitute-if 'nil (lambda (a) (or (equal a '("." . "()")) (equal a '("?" . "()")) (equal a '("!" . "()"))))   (merge-hrefs (seperate-defs text))))

(defvar *test* (clean-text (elt *texts* 0)))

;;AI, to be used after clean-text is applied

(defun get-subject (text)
	(subseq (car text) 1 (- (length (car text)) 2)))

(defun collect-hrefs (text)
      (let ((lst 'nil)
	    (prev 'nil))
	(dolist (crrnt (cdr text))
	  (when crrnt
	    (when (or
		   (and prev 
			(null (equal (car crrnt) "(href)"))
			(upper-case-p (elt (car crrnt) 0)))
		   (and prev(equal (car crrnt) "of")))
	      (push crrnt lst))
	    (when (null (or
			 (and prev 
			      (null (equal (car crrnt) "(href)"))
			      (upper-case-p (elt (car crrnt) 0)))
		 	 (and prev (equal (car crrnt) "of"))))
	      (setq prev 'nil))
			 
	    (when (equal (car crrnt) "(href)")
	      (push crrnt lst)
	      (setq prev 't)))
	  (when (null crrnt)
	    (setq prev 'nil)))
	(reverse lst)))

(defun make-request (&key ((:back-links-request blr) 'nil) ((:random-test-request rtr) 'nil) ((:exit-request ext)'nil) ((:get-site-request gsr) 'nil) ((:get-back-links gbl) 'nil))
      (when blr
	(send-request (format-back-links-request blr)))
      (when rtr
	(send-request "randomTest?"))
      (when gsr
	(send-request (format-get-site-request gsr)))
      (when gbl
	(send-request (format-get-back-links-request gbl)))
      (when ext
	(send-request "exit?")))

(defun format-get-site-request (subject)
	(concatenate 'string "getSite?" "&" subject))

(defun format-get-back-links-request (subject)
	(concatenate 'string "getBackLinks?" "&"  subject))

(defun format-back-links-request (subject-list)
      	(let ((request "getNumBackLinks?"))
	  (dolist (crrnt subject-list)
	  (setq request (concatenate 'string request "&" crrnt)))
	request))

(defun send-request (request)
	(write-request request)
	(run-request))

(defun write-request (request)
	(delete-file "~/QGRequest.txt")
      	(with-open-file (stream "~/QGRequest.txt" :if-exists :overwrite :if-does-not-exist :create :direction :output)
	(format stream request)))

(defun run-request ()
	(ext:run-program "sh" :arguments '("front-end-request.sh")))

(defun check-front-end-topics (subject-list)
      (send-request :type-request subject-list)
      (with-open-file (result "~/result.txt" :direction :input)
	(read-line result)
	(let ((line (read-line result 'nil))
	      (result-list)
	      (topic-list))
	  (while line
	    (push line result-list)
	    (setq line (read-line result 'nil)))
	  (dolist (crrnt result-list)
	    (let ((x 0)
		  (c 'nil))
	      (while (null (equal c #\:))
		(setq c (subseq crrnt x (+ x 1)))
		(setq x (+ x 1)))
	      (push (cons (subseq crrnt 0 x) (subseq crrnt (+ x 1))) topic-list)))
	  topic-list)))

(defun get-result ()
  (let ((final ""))
    (with-open-file (result "~/result.txt")
       (let ((line (read-line result 'nil)))
         (while line
	   (setq final (concatenate 'string final (string #\newline) line))
	   (setq line (read-line result 'nil)))))
  final))

 (defun parse-result ()
      (let* ((result (string-trim '(#\newline) (get-result))) (result-type (get-result-type result)))
	(cond 
	  ((equal result-type "getBackLinks")
	   (back-links-parse result))
	  ((equal result-type "getSite")
	   (get-site-parse result))
	  ((equal result-type "getNumBackLinks")
	   (num-back-links-parse result)))))

(defun get-result-type (result)
   (subseq result 0 (position #\? result)))

(defun request (&key ((:back-links-request blr) 'nil) ((:random-test-request rtr) 'nil) ((:exit-request ext)'nil) ((:get-site-request gsr) 'nil) ((:get-back-links gbl) 'nil))
      (make-request :back-links-request blr :random-test-request rtr :exit-request ext :get-site-request gsr :get-back-links gbl)
      (parse-result))

(defun num-back-links-parse (result)
      (setq result (subseq result (+ (position #\newline result) 1)))
      (let ((result-list 'nil) 
	    (pos-newline (position #\newline result))
	    (pos-colon (position #\: result)))
	(while pos-newline
	  (push (cons (subseq result 0 pos-colon) (parse-integer (subseq result (1+ pos-colon) pos-newline))) result-list)
	  (setq result (subseq result (1+ pos-newline)))
	  (setq pos-newline (position #\newline result))
	  (setq pos-colon (position #\: result)))
	(push (cons (subseq result 0 pos-colon) (parse-integer (subseq result (1+ pos-colon)))) result-list)
	result-list))

(defun back-links-parse (res)
      (let* ((result (subseq res (position #\newline res)))
	     (currentbl (subseq result 0 (position #\newline result)))
	     (restbls (subseq result (+ (position #\newline result) 1)))
	     (ret 'nil))
	(while (position #\newline restbls)
	  (push currentbl ret)
	  currentbl
	  (setq currentbl (subseq restbls 0 (position #\newline restbls)))
	  (setq restbls (subseq restbls (+ (position #\newline restbls) 1))))
	ret))

(defun get-site-parse (result)
   (clean-text result))
